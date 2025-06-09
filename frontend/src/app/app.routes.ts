import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';

export const routes: Routes = [
  // Rotas de Autenticação
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  //{ path: 'recipes', component: RecipesComponent },

  // Rota Padrão: Redireciona para a tela de login se o usuário não estiver logado
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  // Rota "Catch-all": Redireciona para a rota padrão se a URL não for encontrada
  { path: '**', redirectTo: '/login' }
];