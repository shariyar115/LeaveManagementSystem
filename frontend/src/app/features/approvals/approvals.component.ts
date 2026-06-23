import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';

import { LeaveRequestService } from '../../core/services/leave-request.service';
import { LeaveRequest } from '../../core/models/leave.models';
import { statusSeverity } from '../../shared/status.util';

@Component({
  selector: 'app-approvals',
  imports: [
    DatePipe,
    FormsModule,
    TableModule,
    TagModule,
    ButtonModule,
    DialogModule,
    TextareaModule,
    TooltipModule,
  ],
  templateUrl: './approvals.component.html',
})
export class ApprovalsComponent {
  private readonly leaveRequestService = inject(LeaveRequestService);
  private readonly messageService = inject(MessageService);

  readonly pending = signal<LeaveRequest[]>([]);
  readonly selected = signal<LeaveRequest[]>([]);
  readonly loading = signal(false);
  readonly busy = signal(false);

  // Rejection dialog state
  readonly rejectDialogVisible = signal(false);
  readonly rejectComment = signal('');
  private rejectTarget: 'single' | 'bulk' = 'single';
  private rejectId: number | null = null;

  readonly statusSeverity = statusSeverity;

  constructor() {
    this.reload();
  }

  reload(): void {
    this.loading.set(true);
    this.leaveRequestService.getPending().subscribe({
      next: (data) => {
        this.pending.set(data);
        this.selected.set([]);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  approve(request: LeaveRequest): void {
    this.busy.set(true);
    this.leaveRequestService.setApproval(request.id, { status: 'Approved' }).subscribe({
      next: () => {
        this.notify(`Request #${request.id} approved.`);
        this.reload();
        this.busy.set(false);
      },
      error: () => this.busy.set(false),
    });
  }

  openReject(request: LeaveRequest): void {
    this.rejectTarget = 'single';
    this.rejectId = request.id;
    this.rejectComment.set('');
    this.rejectDialogVisible.set(true);
  }

  bulkApprove(): void {
    const ids = this.selected().map((r) => r.id);
    if (!ids.length) return;
    this.busy.set(true);
    this.leaveRequestService.bulkApproval({ requestIds: ids, status: 'Approved' }).subscribe({
      next: () => {
        this.notify(`${ids.length} request(s) approved.`);
        this.reload();
        this.busy.set(false);
      },
      error: () => this.busy.set(false),
    });
  }

  openBulkReject(): void {
    if (!this.selected().length) return;
    this.rejectTarget = 'bulk';
    this.rejectComment.set('');
    this.rejectDialogVisible.set(true);
  }

  confirmReject(): void {
    const comment = this.rejectComment().trim() || null;
    this.busy.set(true);

    const done = (count: number) => {
      this.notify(`${count} request(s) rejected.`);
      this.rejectDialogVisible.set(false);
      this.reload();
      this.busy.set(false);
    };

    if (this.rejectTarget === 'single' && this.rejectId != null) {
      this.leaveRequestService
        .setApproval(this.rejectId, { status: 'Rejected', rejectionComment: comment })
        .subscribe({ next: () => done(1), error: () => this.busy.set(false) });
    } else {
      const ids = this.selected().map((r) => r.id);
      this.leaveRequestService
        .bulkApproval({ requestIds: ids, status: 'Rejected', rejectionComment: comment })
        .subscribe({ next: () => done(ids.length), error: () => this.busy.set(false) });
    }
  }

  private notify(detail: string): void {
    this.messageService.add({ severity: 'success', summary: 'Done', detail });
  }
}
