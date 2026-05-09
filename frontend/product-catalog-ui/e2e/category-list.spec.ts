import { test, expect } from '@playwright/test';

test.describe('Category List', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/categories');
    await page.waitForSelector('table tbody tr');
  });

  test('displays seeded categories', async ({ page }) => {
    await expect(page.locator('table tbody')).toContainText('Electronics');
    await expect(page.locator('table tbody')).toContainText('Laptops');
    await expect(page.locator('table tbody')).toContainText('Phones');
  });

  test('shows parent category name for child categories', async ({ page }) => {
    const rows = page.locator('table tbody tr');

    // Laptops and Phones are children of Electronics
    const laptopsRow = rows.filter({ hasText: 'Laptops' });
    await expect(laptopsRow).toContainText('Electronics');

    const phonesRow = rows.filter({ hasText: 'Phones' });
    await expect(phonesRow).toContainText('Electronics');
  });

  test('top-level category shows dash for parent', async ({ page }) => {
    const electronicsRow = page.locator('table tbody tr').filter({ hasText: 'Electronics' });
    await expect(electronicsRow).toContainText('—');
  });

  test('New Category button reveals create form', async ({ page }) => {
    await page.getByRole('button', { name: '+ New Category' }).click();
    await expect(page.getByRole('heading', { name: 'New Category' })).toBeVisible();
    await expect(page.getByLabel('Name')).toBeVisible();
  });

  test('shows validation error when creating without a name', async ({ page }) => {
    await page.getByRole('button', { name: '+ New Category' }).click();
    await page.getByRole('button', { name: 'Create Category' }).click();
    await expect(page.locator('body')).toContainText('This field is required');
  });

  test('creates a new top-level category and shows it in the table', async ({ page }) => {
    await page.getByRole('button', { name: '+ New Category' }).click();

    await page.getByLabel('Name').fill('E2E Test Category');
    await page.getByLabel('Description').fill('Created by Playwright');

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/api/categories') && r.status() === 201),
      page.getByRole('button', { name: 'Create Category' }).click(),
    ]);

    expect(response.status()).toBe(201);
    await expect(page.locator('table tbody')).toContainText('E2E Test Category');
    await expect(page.locator('table tbody')).toContainText('Created by Playwright');
  });

  test('creates a child category under an existing parent', async ({ page }) => {
    await page.getByRole('button', { name: '+ New Category' }).click();

    await page.getByLabel('Name').fill('E2E Child Category');
    const parentSelect = page.getByLabel('Parent Category');
    await parentSelect.selectOption({ label: 'Electronics' });

    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('/api/categories') && r.status() === 201),
      page.getByRole('button', { name: 'Create Category' }).click(),
    ]);

    expect(response.status()).toBe(201);
    const newRow = page.locator('table tbody tr').filter({ hasText: 'E2E Child Category' });
    await expect(newRow).toContainText('Electronics');
  });

  test('Cancel button hides the form without creating', async ({ page }) => {
    const initialCount = await page.locator('table tbody tr').count();

    await page.getByRole('button', { name: '+ New Category' }).click();
    await page.getByRole('heading', { name: 'New Category' }).waitFor();

    await page.getByRole('button', { name: 'Cancel' }).click();
    await expect(page.getByRole('heading', { name: 'New Category' })).not.toBeVisible();
    await expect(page.locator('table tbody tr')).toHaveCount(initialCount);
  });
});
