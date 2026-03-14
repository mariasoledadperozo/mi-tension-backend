```powershell
# Script para sincronizar repositorio local con GitHub sin perder archivos

Write-Host "Añadiendo cambios..."
git add .

Write-Host "Creando commit..."
git commit -m "sync automatico" 2>$null

Write-Host "Trayendo cambios remotos..."
git pull origin master --allow-unrelated-histories

Write-Host "Subiendo cambios a GitHub..."
git push origin master

Write-Host "Sincronizacion completada"
```
