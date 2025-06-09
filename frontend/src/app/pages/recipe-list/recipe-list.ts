// Em frontend/src/app/pages/recipe-list/recipe-list.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { Recipe } from '../../models/recipe.model';
import { RecipeService } from '../../services/recipe.service';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './recipe-list.html',
  styleUrls: ['./recipe-list.scss']
})
export class RecipeListComponent implements OnInit {
  // Usaremos um Observable com o pipe 'async' no template.
  // Isso simplifica o gerenciamento de inscrições e torna o código mais limpo.
  recipes$!: Observable<Recipe[]>;

  constructor(private recipeService: RecipeService) {}

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.recipes$ = this.recipeService.getAllRecipes();
  }

  deleteRecipe(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta receita?')) {
      this.recipeService.deleteRecipe(id).subscribe({
        next: () => {
          console.log('Receita excluída com sucesso');
          // Recarrega a lista para refletir a exclusão
          this.loadRecipes();
        },
        error: (err) => console.error('Erro ao excluir receita', err)
      });
    }
  }
}