"""
Google Sheets integration for saving test results
"""
import os
import json
from typing import List, Dict

try:
    import gspread
    from oauth2client.service_account import ServiceAccountCredentials
    GSPREAD_AVAILABLE = True
except ImportError:
    GSPREAD_AVAILABLE = False


class GoogleSheetsManager:
    """ניהול שמירה לגיליון Google Sheets"""
    
    def __init__(self, credentials_path: str, sheet_id: str):
        if not GSPREAD_AVAILABLE:
            raise ImportError("gspread is not installed")
        
        self.sheet_id = sheet_id
        self.credentials_path = credentials_path
        self.client = self._authenticate()
        self.spreadsheet = self.client.open_by_key(sheet_id)
    
    def _authenticate(self):
        """התחבר ל-Google Sheets API"""
        scope = ['https://spreadsheets.google.com/feeds',
                 'https://www.googleapis.com/auth/drive']
        
        creds = ServiceAccountCredentials.from_json_keyfile_name(
            self.credentials_path, scope)
        return gspread.authorize(creds)
    
    def create_worksheet(self, title: str):
        """בנה גיליון עבודה חדש"""
        try:
            worksheet = self.spreadsheet.worksheet(title)
            # אם קיים, נקה אותו
            worksheet.clear()
            return worksheet
        except gspread.exceptions.WorksheetNotFound:
            # בנה גיליון חדש
            return self.spreadsheet.add_worksheet(title=title, rows=100, cols=20)
    
    def write_results(self, worksheet_title: str, results: List[Dict]):
        """כתוב תוצאות לגיליון"""
        worksheet = self.create_worksheet(worksheet_title)
        
        if not results:
            return
        
        # כתוב כותרות
        headers = list(results[0].keys())
        worksheet.append_row(headers)
        
        # כתוב נתונים
        for result in results:
            row = [str(result.get(header, "")) for header in headers]
            worksheet.append_row(row)
    
    def write_summary(self, summary_data: Dict):
        """כתוב סיכום של כל האיטרציות"""
        worksheet = self.create_worksheet("Summary")
        
        worksheet.append_row(["Iteration", "Average Score", "Timestamp"])
        for name, data in summary_data.items():
            worksheet.append_row([name, data.get("score", 0), data.get("timestamp", "")])


def save_to_google_sheets(results: List[Dict], iteration_name: str, sheet_id: str = None):
    """שמור תוצאות ל-Google Sheets (אם מוגדרים credentials)"""
    credentials_path = os.getenv("GOOGLE_SHEETS_CREDENTIALS_JSON")
    sheet_id = sheet_id or os.getenv("GOOGLE_SHEETS_ID")
    
    if not (credentials_path and sheet_id and os.path.exists(credentials_path)):
        print(f"⚠ Google Sheets credentials not configured. Skipping upload.")
        return
    
    try:
        manager = GoogleSheetsManager(credentials_path, sheet_id)
        manager.write_results(iteration_name, results)
        print(f"✓ Results saved to Google Sheets: {iteration_name}")
    except Exception as e:
        print(f"✗ Failed to save to Google Sheets: {e}")
