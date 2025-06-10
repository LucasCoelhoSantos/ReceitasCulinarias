export interface Recipe {
  id?: string;
  createdAt?: Date;
  updatedAt?: Date;
  name: string;
  description: string;
  ingredients: string;
  instructions: string;
  prepTimeMinutes: number;
  category: number;
  imageUrl?: string;
}