import { test, expect } from '@playwright/test';

test.describe('Product List', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/products');
    await page.waitForSelector('table tbody tr');
  });

  test('displays seeded products in the table', async ({ page }) => {
    const rows = page.locator('table tbody tr');
    await expect(rows).toHaveCount(3);

    await expect(page.locator('table tbody')).toContainText('MacBook Pro 14');
    await expect(page.locator('table tbody')).toContainText('ThinkPad X1 Carbon');
    await expect(page.locator('table tbody')).toContainText('iPhone 15 Pro');
  });

  test('shows SKUs in monospace cells', async ({ page }) => {
    await expect(page.locator('table tbody')).toContainText('LT-000001');
    await expect(page.locator('table tbody')).toContainText('LT-000002');
    await expect(page.locator('table tbody')).toContainText('PH-000001');
  });

  test('search filters products with debounce', async ({ page }) => {
    const searchInput = page.getByPlaceholder('Search products…');
    await searchInput.fill('macbook');

    // debounce fires after 300ms — wait for network response
    await page.waitForResponse((r) => r.url().includes('/api/products') && r.status() === 200);

    const rows = page.locator('table tbody tr');
    await expect(rows).toHaveCount(1);
    await expect(page.locator('table tbody')).toContainText('MacBook Pro 14');
    await expect(page.locator('table tbody')).not.toContainText('ThinkPad');
  });

  test('search clearing restores full list', async ({ page }) => {
    const searchInput = page.getByPlaceholder('Search products…');
    await searchInput.fill('macbook');
    await page.waitForResponse((r) => r.url().includes('/api/products') && r.status() === 200);

    await searchInput.clear();
    await page.waitForResponse((r) => r.url().includes('/api/products') && r.status() === 200);

    await expect(page.locator('table tbody tr')).toHaveCount(3);
  });

  test('category filter narrows results', async ({ page }) => {
    const categorySelect = page.locator('select');
    // Choose "Phones" category
    await categorySelect.selectOption({ label: 'Phones' });
    await page.waitForResponse((r) => r.url().includes('categoryId') && r.status() === 200);

    await expect(page.locator('table tbody')).toContainText('iPhone 15 Pro');
    await expect(page.locator('table tbody')).not.toContainText('MacBook Pro 14');
  });

  test('category filter reset shows all products', async ({ page }) => {
    const categorySelect = page.locator('select');
    await categorySelect.selectOption({ label: 'Phones' });
    await page.waitForResponse((r) => r.url().includes('categoryId') && r.status() === 200);

    await categorySelect.selectOption({ label: 'All Categories' });
    await page.waitForResponse((r) => r.url().includes('/api/products') && r.status() === 200);

    await expect(page.locator('table tbody tr')).toHaveCount(3);
  });

  test('New Product button navigates to create form', async ({ page }) => {
    await page.getByRole('link', { name: '+ New Product' }).click();
    await expect(page).toHaveURL('/products/new');
    await expect(page.getByRole('heading', { name: 'New Product' })).toBeVisible();
  });

  test('product name link navigates to detail page', async ({ page }) => {
    await page.getByRole('link', { name: 'MacBook Pro 14' }).click();
    await expect(page).toHaveURL(/\/products\/[0-9a-f-]{36}$/);
    await expect(page.getByRole('heading', { name: 'MacBook Pro 14' })).toBeVisible();
  });
});
