# Register
curl.exe -X POST http://localhost:5000/api/v1/identity/register -H "Content-Type: application/json" -d '@register_payload.json'

# Login
curl.exe -X POST http://localhost:5000/api/v1/identity/login -H "Content-Type: application/json" -d '@login_payload.json' > token.json

# Extract Token
$json = Get-Content token.json | ConvertFrom-Json
$token = $json.token

# Change Language
curl.exe -v -X PATCH http://localhost:5000/api/v1/identity/change-language -H "Authorization: Bearer $token" -H "Content-Type: application/json" -d '@change_lang_payload.json'
