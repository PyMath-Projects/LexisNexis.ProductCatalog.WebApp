import { test, expect } from '@playwright/test';

test.describe('Product Edit', () => {
  test('edit form loads existing values', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');

    const editLinks = page.locator('a', { hasText: 'Edit' });
    await editLinks.first().click();

    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}\/edit$/);
    await expect(page.getByRole('heading', { name: 'Edit Product' })).toBeVisible();

    // Name field should be pre-populated
    const nameInput = page.getByLabel('Name');
    const nameValue = await nameInput.inputValue();
    expect(nameValue.length).toBeGreaterThan(0);
  });

  test('SKU field is disabled in edit mode', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');

    await page.locator('a', { hasText: 'Edit' }).first().click();

    // SKU field should not be present (hidden via @if(!isEdit))
    await expect(page.getByLabel('SKU')).not.toBeVisible();
  });

  test('saves updated name and redirects to detail', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');

    await page.locator('a', { hasText: 'Edit' }).first().click();
    await page.waitForSelector('form');

    const nameInput = page.getByLabel('Name');
    await nameInput.clear();
    await nameInput.fill('MacBook Pro 14 (Updated)');

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/api/products/') && r.request().method() === 'PUT' && r.status() === 200),
      page.getByRole('button', { name: 'Save Changes' }).click(),
    ]);

    expect(response.status()).toBe(200);
    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}$/);
    await expect(page.getByRole('heading', { name: 'MacBook Pro 14 (Updated)' })).toBeVisible();
  });

  test('Cancel link returns to product list', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.locator('a', { hasText: 'Edit' }).first().click();

    await page.getByRole('link', { name: 'Cancel' }).click();
    await expect(page).toHaveURL('/products');
  });
});
