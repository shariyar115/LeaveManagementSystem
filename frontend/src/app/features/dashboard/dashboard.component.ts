import { DatePipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressBarModule } from 'primeng/progressbar';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { BalanceService } from '../../core/services/balance.service';
import { LeaveRequestService } from '../../core/services/leave-request.service';
import { LeaveTypeService } from '../../core/services/leave-type.service';
import { AppStateService } from '../../core/services/app-state.service';
import {
  LeaveBalanceSummary,
  LeaveRequest,
  LeaveStatus,
  LeaveType,
} from '../../core/models/leave.models';
import { downloadCsv } from '../../core/utils/csv.util';
import { statusSeverity } from '../../shared/status.util';

@Component({
  selector: 'app-dashboard',
  imports: [
    DatePipe,
    FormsModule,
    TableModule,
    TagModule,
    ButtonModule,
    SelectModule,
    DatePickerModule,
    InputTextModule,
    CardModule,
    ProgressBarModule,
    TooltipModule,
  ],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent {
  private readonly leaveRequestService = inject(LeaveRequestService);
  private readonly leaveTypeService = inject(LeaveTypeService);
  private readonly balanceService = inject(BalanceService);
  private readonly messageService = inject(MessageService);
  readonly state = inject(AppStateService);

  readonly requests = signal<LeaveRequest[]>([]);
  readonly leaveTypes = signal<LeaveType[]>([]);
  readonly summary = signal<LeaveBalanceSummary | null>(null);
  readonly loading = signal(false);

  // Filter state
  readonly statusFilter = signal<LeaveStatus | null>(null);
  readonly typeFilter = signal<number | null>(null);
  readonly dateRange = signal<Date[] | null>(null);
  readonly search = signal('');

  readonly statusSeverity = statusSeverity;

  readonly statusOptions = [
    { label: 'All statuses', value: null },
    { label: 'Pending', value: 'Pending' as LeaveStatus },
    { label: 'Approved', value: 'Approved' as LeaveStatus },
    { label: 'Rejected', value: 'Rejected' as LeaveStatus },
    { label: 'Cancelled', value: 'Cancelled' as LeaveStatus },
  ];

  readonly typeOptions = computed(() => [
    { label: 'All types', value: null },
    ...this.leaveTypes().map((t) => ({ label: t.name, value: t.id })),
  ]);

  readonly filteredRequests = computed(() => {
    const status = this.statusFilter();
    const typeId = this.typeFilter();
    const range = this.dateRange();
    const term = this.search().trim().toLowerCase();
    const [from, to] = range ?? [];

    return this.requests().filter((r) => {
      if (status && r.status !== status) return false;
      if (typeId != null && r.leaveTypeId !== typeId) return false;
      if (from && new Date(r.endDate) < from) return false;
      if (to && new Date(r.startDate) > to) return false;
      if (term && !`${r.leaveTypeName} ${r.reason ?? ''} ${r.status}`.toLowerCase().includes(term))
        return false;
      return true;
    });
  });

  private readonly searchInput$ = new Subject<string>();

  constructor() {
    this.searchInput$
      .pipe(debounceTime(300), takeUntilDestroyed())
      .subscribe((value) => this.search.set(value));

    this.leaveTypeService.getAll().subscribe((types) => this.leaveTypes.set(types));

    // Reload whenever the selected employee changes.
    effect(() => {
      const employeeId = this.state.selectedEmployeeId();
      this.reload(employeeId);
    });
  }

  onSearch(value: string): void {
    this.searchInput$.next(value);
  }

  reload(employeeId: number): void {
    this.loading.set(true);
    this.leaveRequestService.getAll({ employeeId }).subscribe({
      next: (data) => {
        this.requests.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
    this.balanceService.getSummary(employeeId).subscribe((s) => this.summary.set(s));
  }

  usedPercent(balance: number, defaultDays: number): number {
    if (!defaultDays) return 0;
    return Math.min(100, Math.round((balance / defaultDays) * 100));
  }

  cancel(request: LeaveRequest): void {
    this.leaveRequestService.cancel(request.id).subscribe(() => {
      this.messageService.add({
        severity: 'success',
        summary: 'Cancelled',
        detail: `Request #${request.id} cancelled.`,
      });
      this.reload(this.state.selectedEmployeeId());
    });
  }

  exportCsv(): void {
    const rows = this.filteredRequests().map((r) => [
      r.id,
      r.leaveTypeName,
      r.startDate.substring(0, 10),
      r.endDate.substring(0, 10),
      r.daysRequested,
      r.status,
      r.reason ?? '',
      r.rejectionComment ?? '',
    ]);
    downloadCsv(
      `leave-history-${this.state.selectedEmployee()?.name ?? 'employee'}.csv`,
      ['Id', 'Leave Type', 'Start', 'End', 'Days', 'Status', 'Reason', 'Rejection Comment'],
      rows,
    );
  }
}
