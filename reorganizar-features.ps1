# Script para reorganizar ramas con contenido específico de cada feature
# UTF-8 sin BOM

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  REORGANIZANDO RAMAS POR FEATURES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$currentBranch = git branch --show-current
Write-Host "Rama actual: $currentBranch" -ForegroundColor Cyan
Write-Host ""

# ==========================================
# 1. MASTER - Base limpia
# ==========================================
Write-Host "==========================================
" -ForegroundColor Yellow
Write-Host "1. Configurando MASTER (base limpia)" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout master

# Eliminar controladores específicos de master (solo dejar esqueleto)
if (Test-Path "Controllers/UsuariosController.cs") {
    Remove-Item "Controllers/UsuariosController.cs" -Force
}
if (Test-Path "Controllers/RegistrosPresionController.cs") {
    Remove-Item "Controllers/RegistrosPresionController.cs" -Force
}
if (Test-Path "Controllers/RecordatoriosController.cs") {
    Remove-Item "Controllers/RecordatoriosController.cs" -Force
}

# Eliminar DTOs de master
if (Test-Path "DTOs") {
    Remove-Item "DTOs" -Recurse -Force
}

git add -A
git commit -m "Master: Base limpia con esqueleto del proyecto" --allow-empty
git push origin master --force

Write-Host "Master configurado" -ForegroundColor Green
Write-Host ""

# ==========================================
# 2. DEVELOP - Todo el código
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "2. Configurando DEVELOP (código completo)" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout develop

# Asegurarse de que develop tiene TODO
git add -A
git commit -m "Develop: Codigo completo con todos los controladores y DTOs" --allow-empty
git push origin develop --force

Write-Host "Develop configurado" -ForegroundColor Green
Write-Host ""

# ==========================================
# 3. FEATURE/API-USUARIOS - Solo usuarios
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "3. Configurando FEATURE/API-USUARIOS" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout feature/api-usuarios
git checkout master -- .
git checkout develop -- Models/Usuario.cs
git checkout develop -- Enums/Sexo.cs
git checkout develop -- Controllers/UsuariosController.cs

# Crear DTOs de Usuario
New-Item -ItemType Directory -Path "DTOs/Usuario" -Force | Out-Null
git checkout develop -- DTOs/Usuario/

git add -A
git commit -m "Feature: API de Usuarios con DTOs" --allow-empty
git push origin feature/api-usuarios --force

Write-Host "Feature usuarios configurado" -ForegroundColor Green
Write-Host ""

# ==========================================
# 4. FEATURE/API-PRESION - Solo registros presion
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "4. Configurando FEATURE/API-PRESION" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout feature/api-presion
git checkout master -- .
git checkout develop -- Models/RegistroPresion.cs
git checkout develop -- Controllers/RegistrosPresionController.cs

# Crear DTOs de RegistroPresion
New-Item -ItemType Directory -Path "DTOs/RegistroPresion" -Force | Out-Null
git checkout develop -- DTOs/RegistroPresion/

git add -A
git commit -m "Feature: API de Registros de Presion con DTOs" --allow-empty
git push origin feature/api-presion --force

Write-Host "Feature presion configurado" -ForegroundColor Green
Write-Host ""

# ==========================================
# 5. FEATURE/API-RECORDATORIOS - Solo recordatorios
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "5. Configurando FEATURE/API-RECORDATORIOS" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout feature/api-recordatorios
git checkout master -- .
git checkout develop -- Models/Recordatorios.cs
git checkout develop -- Enums/DiasSemana.cs
git checkout develop -- Controllers/RecordatoriosController.cs

# Crear DTOs de Recordatorio
New-Item -ItemType Directory -Path "DTOs/Recordatorio" -Force | Out-Null
git checkout develop -- DTOs/Recordatorio/

git add -A
git commit -m "Feature: API de Recordatorios con DTOs" --allow-empty
git push origin feature/api-recordatorios --force

Write-Host "Feature recordatorios configurado" -ForegroundColor Green
Write-Host ""

# ==========================================
# 6. FEATURES VACIAS - Auth, Analisis, Estadisticas
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "6. Configurando FEATURES VACIAS" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

$emptyFeatures = @("feature/api-auth", "feature/api-analisis", "feature/api-estadisticas")

foreach ($feature in $emptyFeatures) {
    Write-Host "Configurando $feature..." -ForegroundColor Cyan
    git checkout $feature
    git checkout master -- .
    git add -A
    git commit -m "Feature: Preparado para desarrollo" --allow-empty
    git push origin $feature --force
    Write-Host "$feature configurado" -ForegroundColor Green
}

Write-Host ""

# ==========================================
# Volver a rama original
# ==========================================
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "Volviendo a: $currentBranch" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

git checkout $currentBranch

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  REORGANIZACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Estructura de ramas:" -ForegroundColor Cyan
Write-Host "  master: Base limpia (esqueleto)" -ForegroundColor White
Write-Host "  develop: Codigo completo" -ForegroundColor White
Write-Host "  feature/api-usuarios: UsuariosController + DTOs" -ForegroundColor White
Write-Host "  feature/api-presion: RegistrosPresionController + DTOs" -ForegroundColor White
Write-Host "  feature/api-recordatorios: RecordatoriosController + DTOs" -ForegroundColor White
Write-Host "  feature/api-auth: Preparado (vacio)" -ForegroundColor White
Write-Host "  feature/api-analisis: Preparado (vacio)" -ForegroundColor White
Write-Host "  feature/api-estadisticas: Preparado (vacio)" -ForegroundColor White
