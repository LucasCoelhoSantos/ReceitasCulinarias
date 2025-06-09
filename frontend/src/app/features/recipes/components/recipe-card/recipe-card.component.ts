import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Recipe } from '../../models/recipe.model';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './recipe-card.component.html',
  styleUrls: ['./recipe-card.component.scss']
})
export class RecipeCardComponent {
  @Input() recipe!: Recipe;
  @Output() viewRecipeDetails = new EventEmitter<string>();
  @Output() deleteRecipe = new EventEmitter<string>();

  onViewRecipe(): void {
    this.viewRecipeDetails.emit(this.recipe.id);
  }

  onDeleteRecipe(): void {
    this.deleteRecipe.emit(this.recipe.id);
  }
} 