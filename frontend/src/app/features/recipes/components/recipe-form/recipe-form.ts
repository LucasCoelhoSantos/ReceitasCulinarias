import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { HttpErrorResponse } from '@angular/common/http';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './recipe-form.html',
  styleUrls: ['./recipe-form.scss']
})
export class RecipeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private recipeService = inject(RecipeService);

  recipeForm!: FormGroup;
  isEditMode = false;
  recipeId: string | null = null;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.recipeId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.recipeId;

    this.recipeForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', Validators.required],
      ingredients: ['', Validators.required],
      instructions: ['', Validators.required],
      prepTimeMinutes: [0, [Validators.required, Validators.min(1)]],
      category: ['', Validators.required],
      imageUrl: ['']
    });

    if (this.isEditMode && this.recipeId) {
      this.loadRecipeData(this.recipeId);
    }
  }

  loadRecipeData(id: string): void {
    this.recipeService.getRecipeById(id).subscribe({
      next: (recipe) => this.recipeForm.patchValue(recipe),
      error: (err) => this.handleError(err as HttpErrorResponse)
    });
  }

  onSubmit(): void {
    if (this.recipeForm.invalid) {
      return;
    }
    this.errorMessage = null;
    const recipeData = this.recipeForm.value;

    if (this.isEditMode && this.recipeId) {
      this.recipeService.updateRecipe(this.recipeId, recipeData).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err)
      });
    } else {
      this.recipeService.createRecipe(recipeData).subscribe({
        next: (createdRecipe) => this.handleSuccess(createdRecipe),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleSuccess(recipe?: Recipe): void {
    const action = this.isEditMode ? 'atualizada' : 'criada';
    alert(`Receita ${action} com sucesso!`);
    this.router.navigate(['/recipes']);
  }

  private handleError(err: HttpErrorResponse): void {
    console.error('Ocorreu um erro:', err);
    if (err.error) {
      if (typeof err.error === 'string') {
        this.errorMessage = err.error;
      } else if (err.error.errors && Array.isArray(err.error.errors)) {
        this.errorMessage = err.error.errors.join('<br>');
      } else if (err.error.message) {
        this.errorMessage = err.error.message;
      } else {
        this.errorMessage = 'Ocorreu um erro desconhecido.';
      }
    } else {
      this.errorMessage = 'Ocorreu um erro na operação. Por favor, tente novamente.';
    }
  }
}