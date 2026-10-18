output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "frontend_name" {
  value = azurerm_static_web_app.frontend.name
}

output "frontend_url" {
  value = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "api_url" {
  value = "https://${azurerm_container_app.api.ingress[0].fqdn}"
}

output "cosmos_account_name" {
  value = azurerm_cosmosdb_account.main.name
}

output "cosmos_endpoint" {
  value = azurerm_cosmosdb_account.main.endpoint
}
