# Ecommerce Monolith API

## הרצה מהירה
## Endpoints עיקריים

- GET /api/products
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}
- GET /api/inventory
- POST /api/inventory
- PUT /api/inventory/{id}
- DELETE /api/inventory/{id}
- GET /api/orders
- POST /api/orders
- PUT /api/orders/{id}
- DELETE /api/orders/{id}

## דיאגרמה
## בעיות צפויות במונולית

1. קושי להגדיל רכיבים בנפרד (scaling).
2. תלות הדוקה בין רכיבי המערכת (coupling).
3. נקודת כשל אחת (single point of failure).
### דוגמה ל־POST /orders
POST /orders
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}

### דוגמה ל־CorrelationId בלוגים (Seq)
[12:00:00 INF] Handling order 123 (CorrelationId=abc-123)

### דוגמה ל־Cache hit/miss
Cache HIT
Cache MISS
