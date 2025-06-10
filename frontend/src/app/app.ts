import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './features/auth/services/auth.service';
import { ConfirmDialogContainerComponent } from './shared/components/confirm-dialog-container/confirm-dialog-container.component';
import { ConfirmDialogService } from './shared/services/confirm-dialog.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink, ConfirmDialogContainerComponent],
  templateUrl: './app.html',
})
export class App {
  title = 'frontend';
  private authService = inject(AuthService);
  private router = inject(Router);
  private confirmDialogService = inject(ConfirmDialogService);

  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  async logout(): Promise<void> {
    const confirmed = await this.confirmDialogService.showDialog({
      title: 'Confirmar Logout',
      message: 'Tem certeza que deseja sair do sistema?'
    });

    if (confirmed) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }
}