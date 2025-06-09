import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/components/login/login';
import { RegisterComponent } from './features/auth/components/register/register';
import { RecipeListComponent } from './features/recipes/components/recipe-list/recipe-list.component';
import { RecipeFormComponent } from './features/recipes/components/recipe-form/recipe-form.component';
import { RecipeDetailsComponent } from './features/recipes/components/recipe-details/recipe-details.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Rotas de Autenticação
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  // Rotas de Receitas (Protegidas)
  {
    path: 'recipes',
    component: RecipeListComponent,
    canActivate: [authGuard]
  },
  {
    path: 'recipes/new',
    component: RecipeFormComponent,
    canActivate: [authGuard]
  },
  {
    path: 'recipes/edit/:id',
    component: RecipeFormComponent,
    canActivate: [authGuard]
  },
  {
    path: 'recipes/:id',
    component: RecipeDetailsComponent,
    canActivate: [authGuard]
  },

  { path: '', redirectTo: '/recipes', pathMatch: 'full' },

  { path: '**', redirectTo: '/recipes' }
];