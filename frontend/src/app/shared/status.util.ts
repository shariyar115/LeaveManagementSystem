import { LeaveStatus } from '../core/models/leave.models';

type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary';

/** Maps a leave status to a PrimeNG p-tag severity for consistent colouring. */
export function statusSeverity(status: LeaveStatus): TagSeverity {
  switch (status) {
    case 'Approved':
      return 'success';
    case 'Pending':
      return 'warn';
    case 'Rejected':
      return 'danger';
    case 'Cancelled':
      return 'secondary';
    default:
      return 'info';
  }
}
