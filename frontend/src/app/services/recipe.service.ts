import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe } from '../models/recipe.model';

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private apiUrl = '/api/v1/recipes';

  constructor(private http: HttpClient) { }

  getAllRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(this.apiUrl);
  }

  getRecipeById(id: string): Observable<Recipe> {
    return this.http.get<Recipe>(`<span class="math-inline">\{this\.apiUrl\}/</span>{id}`);
  }

  createRecipe(recipeData: Omit<Recipe, 'id' | 'dataDeCriacao' | 'dataDeAlteracao'>): Observable<Recipe> {
    return this.http.post<Recipe>(this.apiUrl, recipeData);
  }

  updateRecipe(id: string, recipeData: Partial<Recipe>): Observable<void> {
    return this.http.put<void>(`<span class="math-inline">\{this\.apiUrl\}/</span>{id}`, recipeData);
  }

  deleteRecipe(id: string): Observable<void> {
    return this.http.delete<void>(`<span class="math-inline">\{this\.apiUrl\}/</span>{id}`);
  }
}