# Script para hacer force push de los cambios del gitignore
# ADVERTENCIA: Esto sobrescribira las ramas remotas

# Colores
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }

Write-Warning "=========================================="
Write-Warning "  ADVERTENCIA: FORCE PUSH EN TODAS LAS RAMAS"
Write-Warning "=========================================="
Write-Warning "Este script hara force push y sobrescribira las ramas remotas."
Write-Warning ""
$respuesta = Read-Host "¿Estas seguro de continuar? (escribe SI para confirmar)"

if ($respuesta -ne "SI") {
    Write-Info "Operacion cancelada."
    exit
}

Write-Info ""
Write-Info "Iniciando force push..."

# Guardar rama actual
$currentBranch = git branch --show-current
Write-Info "Rama actual: $currentBranch"
Write-Info ""

# Lista de ramas
$branches = @("master", "develop", "feature/api-analisis", "feature/api-auth", 
              "feature/api-estadisticas", "feature/api-presion", 
              "feature/api-recordatorios", "feature/api-usuarios")

foreach ($branch in $branches) {
    Write-Info "=========================================="
    Write-Info "Force push en rama: $branch"
    Write-Info "=========================================="
    
    # Cambiar a la rama
    git checkout $branch 2>$null
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "No se pudo cambiar a la rama $branch, saltando..."
        continue
    }
    
    # Force push
    git push origin $branch --force
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Force push exitoso en $branch"
    } else {
        Write-Warning "Error al hacer force push en $branch"
    }
    
    Write-Info ""
}

# Volver a la rama original
Write-Info "Volviendo a la rama original: $currentBranch"
git checkout $currentBranch

Write-Success ""
Write-Success "=========================================="
Write-Success "   FORCE PUSH COMPLETADO"
Write-Success "=========================================="
Write-Info ""
Write-Info "Todas las ramas han sido actualizadas en GitHub."
Write-Info "Los archivos .vs/, bin/, obj/ y appsettings.json"
Write-Info "han sido eliminados del repositorio remoto."
