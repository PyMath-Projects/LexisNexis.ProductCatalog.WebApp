import { test, expect } from '@playwright/test';

test.describe('Product Create', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/products/new');
    await page.waitForSelector('form');
  });

  test('shows create form with all required fields', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'New Product' })).toBeVisible();
    await expect(page.getByLabel('Name')).toBeVisible();
    await expect(page.getByLabel('SKU')).toBeVisible();
    await expect(page.getByLabel('Price')).toBeVisible();
    await expect(page.getByLabel('Initial Stock')).toBeVisible();
    await expect(page.getByLabel('Category')).toBeVisible();
  });

  test('shows validation errors on empty submit', async ({ page }) => {
    await page.getByRole('button', { name: 'Create Product' }).click();
    await expect(page.locator('body')).toContainText('This field is required');
  });

  test('shows validation error for invalid SKU format', async ({ page }) => {
    await page.getByLabel('Name').fill('Test Widget');
    await page.getByLabel('SKU').fill('invalid-sku');
    await page.getByLabel('SKU').blur();
    await expect(page.locator('body')).toContainText('Invalid format');
  });

  test('accepts valid SKU format XX-000000', async ({ page }) => {
    await page.getByLabel('SKU').fill('WG-001234');
    await page.getByLabel('SKU').blur();
    await expect(page.locator('body')).not.toContainText('Invalid format');
  });

  test('successfully creates a product and redirects to detail', async ({ page }) => {
    await page.getByLabel('Name').fill('E2E Test Product');
    await page.getByLabel('SKU').fill('EE-009999');
    await page.getByLabel('Price').fill('49.99');
    await page.getByLabel('Initial Stock').fill('10');

    // Select a category
    const categorySelect = page.getByLabel('Category');
    await categorySelect.selectOption({ index: 1 });

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/api/products') && r.status() === 201),
      page.getByRole('button', { name: 'Create Product' }).click(),
    ]);

    expect(response.status()).toBe(201);
    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}$/);
    await expect(page.getByRole('heading', { name: 'E2E Test Product' })).toBeVisible();
  });

  test('Cancel link navigates back to product list', async ({ page }) => {
    await page.getByRole('link', { name: 'Cancel' }).click();
    await expect(page).toHaveURL('/products');
  });
});
