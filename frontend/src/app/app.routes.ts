import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    title: 'Employee Dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
  },
  {
    path: 'apply',
    title: 'Apply for Leave',
    loadComponent: () =>
      import('./features/apply-leave/apply-leave.component').then((m) => m.ApplyLeaveComponent),
  },
  {
    path: 'approvals',
    title: 'Leave Approvals',
    loadComponent: () =>
      import('./features/approvals/approvals.component').then((m) => m.ApprovalsComponent),
  },
  {
    path: 'leave-types',
    title: 'Leave Types (HR)',
    loadComponent: () =>
      import('./features/leave-types/leave-types.component').then((m) => m.LeaveTypesComponent),
  },
  {
    path: 'settlements',
    title: 'Settlements (HR)',
    loadComponent: () =>
      import('./features/settlements/settlements.component').then((m) => m.SettlementsComponent),
  },
  { path: '**', redirectTo: 'dashboard' },
];
