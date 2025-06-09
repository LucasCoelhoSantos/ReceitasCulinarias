export interface Recipe {
  id?: string;
  name: string;
  description: string;
  ingredients: string[];
  instructions: string;
  prepTimeMinutes: number;
  category: number;
  imageUrl?: string;
  createdAt?: Date;
  updatedAt?: Date;
}