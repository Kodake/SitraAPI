$json = Get-Content token.json | ConvertFrom-Json
$token = $json.token
curl.exe -v -X PATCH http://localhost:5000/api/v1/identity/change-language -H "Authorization: Bearer $token" -H "Content-Type: application/json" -d '@change_lang_payload.json'
