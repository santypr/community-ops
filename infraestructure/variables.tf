variable "subscription_id" {
  type        = string
  description = "Suscripción Azure de destino."
}

variable "name_prefix" {
  type        = string
  description = "Prefijo globalmente único para los recursos (Cosmos incluido)."

  validation {
    condition     = can(regex("^[a-z][a-z0-9-]{2,19}$", var.name_prefix))
    error_message = "Usa entre 3 y 20 caracteres: minúsculas, números y guiones; empieza por una letra."
  }
}

variable "location" {
  type        = string
  description = "Región de Container Apps y Cosmos DB."
  default     = "westeurope"
}

variable "static_web_app_location" {
  type        = string
  description = "Región compatible con Azure Static Web Apps."
  default     = "westeurope"
}

variable "api_image" {
  type        = string
  description = "Imagen pública de CommunityOps.Api, con tag inmutable o digest; no usar latest."

  validation {
    condition     = can(regex("(:[^/:]+|@sha256:[a-f0-9]{64})$", var.api_image)) && !endswith(var.api_image, ":latest")
    error_message = "Indica una imagen con tag explícito distinto de latest o digest sha256."
  }
}

variable "cosmos_free_tier_enabled" {
  type        = bool
  description = "Solo se admite una cuenta Cosmos free tier por suscripción."
  default     = true
}
