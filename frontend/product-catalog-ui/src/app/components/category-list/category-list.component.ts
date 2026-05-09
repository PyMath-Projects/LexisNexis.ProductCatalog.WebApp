import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-list.component.html',
})
export class CategoryListComponent implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly fb = inject(FormBuilder);
  private readonly destroy$ = new Subject<void>();

  categories: Category[] = [];
  loading = true;
  errorMessage = '';
  showForm = false;
  submitting = false;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    parentCategoryId: [''],
  });

  ngOnInit(): void {
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCategories(): void {
    this.loading = true;
    this.categoryService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (cats) => {
          this.categories = cats;
          this.loading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to load categories.';
          this.loading = false;
        },
      });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    this.errorMessage = '';
    const raw = this.form.getRawValue();
    this.categoryService
      .create({
        name: raw.name ?? '',
        description: raw.description || null,
        parentCategoryId: raw.parentCategoryId || null,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (cat) => {
          this.categories = [...this.categories, cat];
          this.form.reset({ name: '', description: '', parentCategoryId: '' });
          this.showForm = false;
          this.submitting = false;
        },
        error: () => {
          this.errorMessage = 'Failed to create category.';
          this.submitting = false;
        },
      });
  }

  parentName(parentId: string | null): string {
    if (!parentId) return '—';
    return this.categories.find((c) => c.id === parentId)?.name ?? '—';
  }

  fieldError(name: string): string {
    const ctrl = this.form.get(name);
    if (!ctrl || !ctrl.invalid || !ctrl.touched) return '';
    if (ctrl.errors?.['required']) return 'This field is required.';
    if (ctrl.errors?.['maxlength']) return 'Value is too long.';
    return 'Invalid value.';
  }
}
