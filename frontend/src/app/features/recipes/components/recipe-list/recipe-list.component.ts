import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';
import { RecipeCardComponent } from '../recipe-card/recipe-card.component';

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
  
  recipes$!: Observable<Recipe[]>;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.errorMessage = null;
    this.recipes$ = this.recipeService.getAllRecipes().pipe(
      catchError(error => {
        this.errorMessage = 'Erro ao carregar receitas. Por favor, tente novamente.';
        console.error('Erro ao carregar receitas:', error);
        return of([]);
      })
    );
  }

  onViewRecipeDetails(id: string): void {
    this.router.navigate(['/recipes', id]);
  }

  onDeleteRecipe(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta receita?')) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => this.loadRecipes(),
        error: (error) => {
          this.errorMessage = 'Erro ao excluir receita. Por favor, tente novamente.';
          console.error('Erro ao excluir receita:', error);
        }
      });
    }
  }
} 