import {
  Component,
  OnInit,
  OnDestroy,
  inject,
} from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  Subject,
  combineLatest,
  startWith,
  switchMap,
  debounceTime,
  distinctUntilChanged,
  catchError,
  of,
  takeUntil,
  map,
} from 'rxjs';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ProductSummary } from '../../models/product.model';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-list.component.html',
})
export class ProductListComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  searchControl = new FormControl('', { nonNullable: true });
  categoryControl = new FormControl('', { nonNullable: true });

  products: ProductSummary[] = [];
  categories: Category[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (cats) => (this.categories = cats) });

    combineLatest([
      this.searchControl.valueChanges.pipe(
        startWith(''),
        debounceTime(300),
        distinctUntilChanged()
      ),
      this.categoryControl.valueChanges.pipe(startWith('')),
    ])
      .pipe(
        takeUntil(this.destroy$),
        map(([search, categoryId]) => ({ search, categoryId })),
        switchMap(({ search, categoryId }) => {
          this.loading = true;
          this.errorMessage = '';
          return this.productService
            .getProducts(search || undefined, categoryId || undefined)
            .pipe(
              catchError(() => {
                this.errorMessage = 'Failed to load products.';
                return of([]);
              })
            );
        })
      )
      .subscribe((products) => {
        this.products = products;
        this.loading = false;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  deleteProduct(id: string): void {
    if (!confirm('Delete this product?')) return;
    this.productService
      .delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.products = this.products.filter((p) => p.id !== id);
        },
        error: () => {
          this.errorMessage = 'Failed to delete product.';
        },
      });
  }

  categoryName(id: string): string {
    return this.categories.find((c) => c.id === id)?.name ?? '—';
  }

  statusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'active': return 'bg-green-100 text-green-800';
      case 'discontinued': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
