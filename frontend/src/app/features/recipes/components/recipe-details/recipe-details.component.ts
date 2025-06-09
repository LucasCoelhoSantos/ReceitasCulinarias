import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './recipe-details.component.html',
  styleUrls: ['./recipe-details.component.scss']
})
export class RecipeDetailsComponent implements OnInit {
  route = inject(ActivatedRoute);
  private router = inject(Router);
  private recipeService = inject(RecipeService);

  recipe$!: Observable<Recipe>;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/recipes']);
      return;
    }

    this.loadRecipe(id);
  }

  loadRecipe(id: string): void {
    this.errorMessage = null;
    this.recipe$ = this.recipeService.getRecipeById(id).pipe(
      catchError(error => {
        this.errorMessage = 'Erro ao carregar detalhes da receita. Por favor, tente novamente.';
        console.error('Erro ao carregar detalhes da receita:', error);
        return of({} as Recipe);
      })
    );
  }

  onDeleteRecipe(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta receita?')) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => this.router.navigate(['/recipes']),
        error: (error) => {
          this.errorMessage = 'Erro ao excluir receita. Por favor, tente novamente.';
          console.error('Erro ao excluir receita:', error);
        }
      });
    }
  }
} 