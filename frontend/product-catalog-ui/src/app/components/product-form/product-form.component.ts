import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
})
export class ProductFormComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly destroy$ = new Subject<void>();

  isEdit = false;
  productId = '';
  categories: Category[] = [];
  loading = false;
  submitting = false;
  errorMessage = '';

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    sku: ['', [Validators.required, Validators.pattern(/^[A-Z]{2}-\d{6}$/)]],
    price: [0 as number, [Validators.required, Validators.min(0.01)]],
    currency: ['USD', Validators.required],
    initialQuantity: [0 as number, [Validators.required, Validators.min(0)]],
    categoryId: ['', Validators.required],
  });

  get skuControl(): AbstractControl {
    return this.form.controls.sku;
  }

  ngOnInit(): void {
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (cats) => (this.categories = cats) });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.productId = id;
      this.loading = true;
      this.form.controls.sku.disable();
      this.form.controls.initialQuantity.clearValidators();
      this.form.controls.initialQuantity.updateValueAndValidity();

      this.productService.getById(id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (product) => {
            this.form.patchValue({
              name: product.name,
              description: product.description ?? '',
              sku: product.sku,
              price: product.price,
              currency: product.currency,
              categoryId: product.categoryId,
            });
            this.loading = false;
          },
          error: () => {
            this.errorMessage = 'Failed to load product.';
            this.loading = false;
          },
        });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    const raw = this.form.getRawValue();

    if (this.isEdit) {
      this.productService
        .update(this.productId, {
          name: raw.name,
          description: raw.description || null,
          price: raw.price,
          currency: raw.currency,
          categoryId: raw.categoryId,
        })
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (product) => this.router.navigate(['/products', product.id]),
          error: () => {
            this.errorMessage = 'Failed to update product.';
            this.submitting = false;
          },
        });
    } else {
      this.productService
        .create({
          name: raw.name,
          description: raw.description || null,
          sku: raw.sku,
          price: raw.price,
          currency: raw.currency,
          initialQuantity: raw.initialQuantity,
          categoryId: raw.categoryId,
        })
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (product) => this.router.navigate(['/products', product.id]),
          error: () => {
            this.errorMessage = 'Failed to create product.';
            this.submitting = false;
          },
        });
    }
  }

  fieldError(name: keyof typeof this.form.controls): string {
    const ctrl = this.form.controls[name];
    if (!ctrl.invalid || !ctrl.touched) return '';
    if (ctrl.errors?.['required']) return 'This field is required.';
    if (ctrl.errors?.['maxlength']) return 'Value is too long.';
    if (ctrl.errors?.['min']) return 'Value must be positive.';
    if (ctrl.errors?.['pattern']) return 'Invalid format (expected XX-000000).';
    return 'Invalid value.';
  }
}
