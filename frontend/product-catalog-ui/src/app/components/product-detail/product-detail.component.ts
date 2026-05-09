import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subject, takeUntil, switchMap } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './product-detail.component.html',
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly destroy$ = new Subject<void>();

  product: Product | null = null;
  categories: Category[] = [];
  loading = true;
  errorMessage = '';
  stockDeltaControl = new FormControl(0, { nonNullable: true });
  adjustingStock = false;
  discontinuing = false;

  ngOnInit(): void {
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (cats) => (this.categories = cats) });

    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        switchMap((params) => {
          const id = params.get('id') ?? '';
          return this.productService.getById(id);
        })
      )
      .subscribe({
        next: (product) => {
          this.product = product;
          this.loading = false;
        },
        error: () => {
          this.errorMessage = 'Product not found.';
          this.loading = false;
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  categoryName(): string {
    if (!this.product) return '—';
    return this.categories.find((c) => c.id === this.product!.categoryId)?.name ?? '—';
  }

  adjustStock(): void {
    const delta = this.stockDeltaControl.value;
    if (!this.product || delta === 0) return;
    this.adjustingStock = true;
    this.productService
      .adjustStock(this.product.id, delta)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.product!.quantity += delta;
          this.stockDeltaControl.setValue(0);
          this.adjustingStock = false;
        },
        error: () => {
          this.errorMessage = 'Failed to adjust stock.';
          this.adjustingStock = false;
        },
      });
  }

  discontinue(): void {
    if (!this.product || !confirm('Discontinue this product?')) return;
    this.discontinuing = true;
    this.productService
      .discontinue(this.product.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.product!.status = 'Discontinued';
          this.discontinuing = false;
        },
        error: () => {
          this.errorMessage = 'Failed to discontinue product.';
          this.discontinuing = false;
        },
      });
  }

  deleteProduct(): void {
    if (!this.product || !confirm('Delete this product?')) return;
    this.productService
      .delete(this.product.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => this.router.navigate(['/products']),
        error: () => { this.errorMessage = 'Failed to delete product.'; },
      });
  }

  statusClass(): string {
    switch (this.product?.status?.toLowerCase()) {
      case 'active': return 'bg-green-100 text-green-800';
      case 'discontinued': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
