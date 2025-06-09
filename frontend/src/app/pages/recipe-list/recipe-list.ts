import { Component, inject } from '@angular/core';
import { CommonModule, SlicePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { Recipe } from '../../models/recipe.model';
import { RecipeService } from '../../services/recipe.service';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [CommonModule, RouterModule, SlicePipe], 
  templateUrl: './recipe-list.html',
  styleUrls: ['./recipe-list.scss']
})
export class RecipeListComponent {
  private recipeService = inject(RecipeService);

  public recipes$: Observable<Recipe[]> = this.recipeService.getAllRecipes();

  deleteRecipe(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta receita?')) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => {
          alert('Receita excluída com sucesso!');
          this.recipes$ = this.recipeService.getAllRecipes();
        },
        error: (err) => {
          console.error('Erro ao excluir receita', err);
          alert('Não foi possível excluir a receita.');
        }
      });
    }
  }
}