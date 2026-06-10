from playwright.sync_api import sync_playwright

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()

    def log_console_message(msg):
        print(f"Browser console: {msg.text}")

    page.on("console", log_console_message)

    page.goto("http://localhost:5173")
    page.click("text=Create User")
    page.click("text=Create Order")
    page.screenshot(path="jules-scratch/verification/verification.png")
    browser.close()

with sync_playwright() as playwright:
    run(playwright)