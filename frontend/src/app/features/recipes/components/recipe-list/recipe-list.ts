import { Component, inject } from '@angular/core';
import { CommonModule, SlicePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [CommonModule, RouterModule, SlicePipe],
  templateUrl: './recipe-list.html',
  styleUrls: ['./recipe-list.scss']
})
export class RecipeListComponent {
  private recipeService = inject(RecipeService);
  public recipes$!: Observable<Recipe[]>;
  public errorMessage: string | null = null;

  constructor() {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.errorMessage = null;
    this.recipes$ = this.recipeService.getAllRecipes().pipe(
      catchError(error => {
        this.errorMessage = 'Não foi possível carregar as receitas. Por favor, tente novamente mais tarde.';
        console.error('Erro ao carregar receitas:', error);
        return of([]);
      })
    );
  }

  deleteRecipe(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta receita?')) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => {
          alert('Receita excluída com sucesso!');
          this.loadRecipes();
        },
        error: (err) => {
          console.error('Erro ao excluir receita', err);
          alert('Não foi possível excluir a receita.');
        }
      });
    }
  }
}