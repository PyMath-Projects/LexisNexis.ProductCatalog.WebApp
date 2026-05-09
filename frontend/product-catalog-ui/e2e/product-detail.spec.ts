import { test, expect } from '@playwright/test';

test.describe('Product Detail', () => {
  test('navigates to detail and shows product data', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');

    await page.getByRole('link', { name: 'MacBook Pro 14' }).click();
    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}$/);

    await expect(page.getByRole('heading', { name: 'MacBook Pro 14' })).toBeVisible();
    await expect(page.locator('body')).toContainText('LT-000001');
    await expect(page.locator('body')).toContainText('1,999.99');
    await expect(page.locator('body')).toContainText('15');
    await expect(page.locator('body')).toContainText('Laptops');
    await expect(page.locator('body')).toContainText('Active');
  });

  test('Edit button navigates to product edit form', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.getByRole('link', { name: 'MacBook Pro 14' }).click();

    await page.getByRole('link', { name: 'Edit' }).click();
    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}\/edit$/);
    await expect(page.getByRole('heading', { name: 'Edit Product' })).toBeVisible();
  });

  test('stock adjustment updates quantity', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.getByRole('link', { name: 'ThinkPad X1 Carbon' }).click();

    // Record current quantity text
    await expect(page.locator('body')).toContainText('8');

    const deltaInput = page.locator('input[type="number"]').first();
    await deltaInput.fill('5');

    const applyButton = page.getByRole('button', { name: 'Apply' });
    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/stock') && r.status() === 204),
      applyButton.click(),
    ]);

    expect(response.status()).toBe(204);
    await expect(page.locator('body')).toContainText('13');
  });

  test('negative stock adjustment reduces quantity', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.getByRole('link', { name: 'iPhone 15 Pro' }).click();

    await expect(page.locator('body')).toContainText('25');

    const deltaInput = page.locator('input[type="number"]').first();
    await deltaInput.fill('-5');

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/stock') && r.status() === 204),
      page.getByRole('button', { name: 'Apply' }).click(),
    ]);

    expect(response.status()).toBe(204);
    await expect(page.locator('body')).toContainText('20');
  });

  test('discontinue product changes status badge', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.getByRole('link', { name: 'ThinkPad X1 Carbon' }).click();

    page.once('dialog', (dialog) => dialog.accept());

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/discontinue') && r.status() === 204),
      page.getByRole('button', { name: 'Discontinue Product' }).click(),
    ]);

    expect(response.status()).toBe(204);
    await expect(page.locator('body')).toContainText('Discontinued');
    await expect(page.getByRole('button', { name: 'Discontinue Product' })).not.toBeVisible();
  });

  test('back link returns to product list', async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
    await page.getByRole('link', { name: 'MacBook Pro 14' }).click();

    await page.getByRole('link', { name: '← Products' }).click();
    await expect(page).toHaveURL('/products');
  });
});
