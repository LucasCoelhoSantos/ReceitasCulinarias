import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './recipe-form.component.html',
  styleUrls: ['./recipe-form.component.scss']
})
export class RecipeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private recipeService = inject(RecipeService);
  private confirmDialogService = inject(ConfirmDialogService);

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

  async onSubmit(): Promise<void> {
    if (this.recipeForm.invalid) {
      return;
    }

    const action = this.isEditMode ? 'salvar as alterações' : 'criar a receita';
    const confirmed = await this.confirmDialogService.showDialog({
      title: 'Confirmar ação',
      message: `Tem certeza que deseja ${action}?`
    });

    if (!confirmed) {
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
    this.router.navigate(['/recipes']);
  }

  private handleError(error: HttpErrorResponse): void {
    console.error('Erro ao salvar receita:', error);
    if (error.error?.message) {
      this.errorMessage = error.error.message;
    } else if (error.error?.errors) {
      // Se houver múltiplos erros de validação
      const errorMessages = Object.values(error.error.errors).flat();
      this.errorMessage = errorMessages.join('\n');
    } else {
      this.errorMessage = 'Ocorreu um erro ao salvar a receita. Por favor, tente novamente.';
    }
  }
} 