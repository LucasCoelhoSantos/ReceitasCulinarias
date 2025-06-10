import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mt-4" *ngIf="recipe">
      <div class="row">
        <div class="col-md-8 offset-md-2">
          <div class="card">
            <img [src]="recipe.imageUrl || 'assets/images/default-recipe.jpg'" 
                 class="card-img-top" 
                 [alt]="recipe.name"
                 style="height: 300px; object-fit: cover;">
            <div class="card-body">
              <h2 class="card-title">{{ recipe.name }}</h2>
              <p class="card-text">{{ recipe.description }}</p>
              
              <div class="mb-4">
                <h4>Ingredientes</h4>
                <p>{{ recipe.ingredients }}</p>
              </div>

              <div class="mb-4">
                <h4>Modo de Preparo</h4>
                <p>{{ recipe.instructions }}</p>
              </div>

              <div class="d-flex justify-content-between align-items-center">
                <div>
                  <span class="badge bg-primary me-2">
                    <i class="bi bi-clock"></i> {{ recipe.prepTimeMinutes }} minutos
                  </span>
                  <span class="badge bg-secondary">
                    <i class="bi bi-tag"></i> {{ getCategoryName(recipe.category) }}
                  </span>
                </div>
                <div>
                  <button class="btn btn-primary me-2" [routerLink]="['/recipes/edit', recipe.id]">
                    <i class="bi bi-pencil"></i> Editar
                  </button>
                  <button class="btn btn-danger" (click)="onDelete()">
                    <i class="bi bi-trash"></i> Excluir
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    .card-title {
      color: #333;
      margin-bottom: 1rem;
    }
    .badge {
      font-size: 0.9rem;
      padding: 0.5rem 1rem;
    }
  `]
})
export class RecipeDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private recipeService = inject(RecipeService);
  private confirmDialogService = inject(ConfirmDialogService);

  recipe: Recipe | null = null;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadRecipe(id);
    }
  }

  private loadRecipe(id: string): void {
    this.recipeService.getRecipeById(id).subscribe({
      next: (recipe: Recipe) => {
        this.recipe = recipe;
      },
      error: (error: Error) => {
        console.error('Erro ao carregar receita:', error);
        this.router.navigate(['/recipes']);
      }
    });
  }

  async onDelete(): Promise<void> {
    if (!this.recipe?.id) return;

    const confirmed = await this.confirmDialogService.showDialog({
      title: 'Confirmar exclusão',
      message: 'Tem certeza que deseja excluir esta receita? Esta ação não pode ser desfeita.'
    });

    if (confirmed) {
      this.recipeService.deleteRecipe(this.recipe.id).subscribe({
        next: () => {
          this.router.navigate(['/recipes']);
        },
        error: (error: Error) => {
          console.error('Erro ao excluir receita:', error);
        }
      });
    }
  }

  getCategoryName(categoryId: number): string {
    const categories: { [key: string]: string } = {
      '1': 'Sobremesas',
      '2': 'Pratos Principais',
      '3': 'Entradas',
      '4': 'Bebidas'
    };
    return categories[categoryId] || 'Categoria desconhecida';
  }
} 