export interface Category {
  id: string;
  name: string;
  description: string | null;
  parentCategoryId: string | null;
  createdAt: string;
}

export interface CategoryTree {
  id: string;
  name: string;
  description: string | null;
  children: CategoryTree[];
}

export interface CreateCategoryRequest {
  name: string;
  description: string | null;
  parentCategoryId: string | null;
}
