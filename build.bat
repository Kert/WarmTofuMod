dotnet build --configuration Release
COPY ".\bin\Release\netstandard2.0\WarmTofuMod.dll" "C:\Program Files (x86)\Steam\steamapps\common\Initial Drift Online\BepInEx\plugins\WarmTofuMod.dll" /Y
pause