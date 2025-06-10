import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { ConfirmDialogService, ConfirmDialogData } from '../../services/confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog-container',
  standalone: true,
  imports: [CommonModule, ConfirmDialogComponent],
  template: `
    <app-confirm-dialog
      [isOpen]="isOpen"
      [title]="dialogData?.title || 'Confirmar ação'"
      [message]="dialogData?.message || 'Tem certeza que deseja realizar esta ação?'"
      (confirm)="onConfirm()"
      (cancel)="onCancel()"
    ></app-confirm-dialog>
  `
})
export class ConfirmDialogContainerComponent implements OnInit, OnDestroy {
  isOpen = false;
  dialogData: ConfirmDialogData | null = null;
  private subscription: Subscription | null = null;

  constructor(private confirmDialogService: ConfirmDialogService) {}

  ngOnInit() {
    this.subscription = this.confirmDialogService.getDialogData().subscribe(data => {
      this.dialogData = data;
      this.isOpen = true;
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  onConfirm() {
    this.isOpen = false;
    this.confirmDialogService.confirm();
  }

  onCancel() {
    this.isOpen = false;
    this.confirmDialogService.cancel();
  }
} 