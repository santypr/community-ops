locals {
  tags = {
    project    = "community-ops"
    managed_by = "terraform"
  }
}

resource "azurerm_resource_group" "main" {
  name     = "${var.name_prefix}-rg"
  location = var.location
  tags     = local.tags
}

resource "azurerm_static_web_app" "frontend" {
  name                = "${var.name_prefix}-web"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.static_web_app_location
  sku_tier            = "Free"
  sku_size            = "Free"
  tags                = local.tags
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "${var.name_prefix}-logs"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.tags
}

resource "azurerm_container_app_environment" "main" {
  name                       = "${var.name_prefix}-environment"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
  tags                       = local.tags
}

resource "azurerm_cosmosdb_account" "main" {
  name                          = "${var.name_prefix}-cosmos"
  location                      = azurerm_resource_group.main.location
  resource_group_name           = azurerm_resource_group.main.name
  offer_type                    = "Standard"
  kind                          = "GlobalDocumentDB"
  free_tier_enabled             = var.cosmos_free_tier_enabled
  local_authentication_disabled = true
  public_network_access_enabled = true
  minimal_tls_version           = "Tls12"
  tags                          = local.tags

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = azurerm_resource_group.main.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "main" {
  name                = "community-ops"
  resource_group_name = azurerm_resource_group.main.name
  account_name        = azurerm_cosmosdb_account.main.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "events" {
  name                  = "events"
  resource_group_name   = azurerm_resource_group.main.name
  account_name          = azurerm_cosmosdb_account.main.name
  database_name         = azurerm_cosmosdb_sql_database.main.name
  partition_key_paths   = ["/communityId"]
  partition_key_version = 2
}

resource "azurerm_container_app" "api" {
  name                         = "${var.name_prefix}-api"
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Single"
  tags                         = local.tags

  identity {
    type = "SystemAssigned"
  }

  ingress {
    external_enabled           = true
    target_port                = 8080
    allow_insecure_connections = false
    transport                  = "http"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name   = "api"
      image  = var.api_image
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }
      env {
        name  = "Events__Store"
        value = "Cosmos"
      }
      env {
        name  = "Cosmos__Endpoint"
        value = azurerm_cosmosdb_account.main.endpoint
      }
      env {
        name  = "Cosmos__Database"
        value = azurerm_cosmosdb_sql_database.main.name
      }
      env {
        name  = "Cosmos__Container"
        value = azurerm_cosmosdb_sql_container.events.name
      }
      env {
        name  = "Cors__AllowedOrigins__0"
        value = "https://${azurerm_static_web_app.frontend.default_host_name}"
      }

      liveness_probe {
        transport = "HTTP"
        port      = 8080
        path      = "/health"
      }
    }
  }
}

resource "azurerm_cosmosdb_sql_role_assignment" "api_reader" {
  resource_group_name = azurerm_resource_group.main.name
  account_name        = azurerm_cosmosdb_account.main.name
  principal_id        = azurerm_container_app.api.identity[0].principal_id
  role_definition_id  = "${azurerm_cosmosdb_account.main.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000001"
  scope               = azurerm_cosmosdb_account.main.id
}
