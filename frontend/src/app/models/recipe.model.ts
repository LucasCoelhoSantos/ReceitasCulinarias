export interface Recipe {
  id: string;
  createdDate: Date;
  updatedDate?: Date;
  name: string;
  description: string;
  ingredients: string;
  instructions: string;
  prepTimeMinutes: number;
  category: string;
  imageUrl?: string;
}