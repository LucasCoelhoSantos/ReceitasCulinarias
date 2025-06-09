import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { RecipeListComponent } from './pages/recipe-list/recipe-list';
import { RecipeFormComponent } from './pages/recipe-form/recipe-form';
import { authGuard } from './guards/auth.guard';

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

  { path: '', redirectTo: '/recipes', pathMatch: 'full' },

  { path: '**', redirectTo: '/recipes' }
];