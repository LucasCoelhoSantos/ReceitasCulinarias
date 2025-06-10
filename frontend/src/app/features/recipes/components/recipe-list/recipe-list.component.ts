import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';
import { RecipeCardComponent } from '../recipe-card/recipe-card.component';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [CommonModule, RouterModule, RecipeCardComponent],
  templateUrl: './recipe-list.component.html',
  styleUrls: ['./recipe-list.component.scss']
})
export class RecipeListComponent implements OnInit {
  private recipeService = inject(RecipeService);
  private router = inject(Router);
  private confirmDialogService = inject(ConfirmDialogService);
  
  recipes$!: Observable<Recipe[]>;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.errorMessage = null;
    this.recipes$ = this.recipeService.getAllRecipes().pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Erro ao carregar receitas:', error);
        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors) {
          const errorMessages = Object.values(error.error.errors).flat();
          this.errorMessage = errorMessages.join('\n');
        } else {
          this.errorMessage = 'Erro ao carregar receitas. Por favor, tente novamente.';
        }
        return of([]);
      })
    );
  }

  onViewRecipeDetails(id: string): void {
    this.router.navigate(['/recipes', id]);
  }

  async onDeleteRecipe(id: string): Promise<void> {
    const confirmed = await this.confirmDialogService.showDialog({
      title: 'Confirmar exclusão',
      message: 'Tem certeza que deseja excluir esta receita? Esta ação não pode ser desfeita.'
    });

    if (confirmed) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => this.loadRecipes(),
        error: (error: HttpErrorResponse) => {
          console.error('Erro ao excluir receita:', error);
          if (error.error?.message) {
            this.errorMessage = error.error.message;
          } else if (error.error?.errors) {
            const errorMessages = Object.values(error.error.errors).flat();
            this.errorMessage = errorMessages.join('\n');
          } else {
            this.errorMessage = 'Erro ao excluir receita. Por favor, tente novamente.';
          }
        }
      });
    }
  }
} 