# החלטת מסד נתונים עבור NotificationService

בחרנו ב-SQLite עבור NotificationService כי:
- השירות קטן, לא דורש ביצועים גבוהים.
- אין צורך ב-DB חיצוני, מספיק קובץ מקומי.
- קל לפריסה ולבדיקות.

אלטרנטיבות: InMemory, LiteDB.