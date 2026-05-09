import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Category,
  CategoryTree,
  CreateCategoryRequest,
} from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/categories';

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.base);
  }

  getCategoryTree(): Observable<CategoryTree[]> {
    return this.http.get<CategoryTree[]>(`${this.base}/tree`);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.base, request);
  }
}
