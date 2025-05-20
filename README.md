

🛡️ TokenServiceProvider
Detta är ett delsystem som hanterar generering och validering av JWT-tokens. Det är en fristående backend-API som körs live på Render och kan användas av resten av gruppens projekt.

📌 Funktioner
/api/Auth/token – POST

Genererar en JWT-token med userId, valfri email och role.

/api/ValidateToken – POST

Validerar att en token är giltig och att userId matchar innehållet i token.

🔧 Användning (för frontend eller andra delsystem)
1. Generera token:
Endpoint:
POST https://tokenserviceprovider.onrender.com/api/Auth/token


Sekvensdiagram – Tokenvalidering
![image](https://github.com/user-attachments/assets/f708bc24-a912-475b-8d6e-f54ee5cf09e0)

📄 Aktivitetsdiagram – Tokenvalidering
![image](https://github.com/user-attachments/assets/de5a8b32-a5d3-40a5-9dcd-7c13aea10974)
