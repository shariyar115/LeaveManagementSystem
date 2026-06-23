export type LeaveStatus = 'Pending' | 'Approved' | 'Rejected' | 'Cancelled';

export interface Employee {
  id: number;
  name: string;
  hireDate: string;
}

export interface LeaveType {
  id: number;
  name: string;
  defaultDays: number;
  isAccrued: boolean;
  accrualRatePerMonth?: number | null;
}

export interface UpsertLeaveType {
  name: string;
  defaultDays: number;
  isAccrued: boolean;
  accrualRatePerMonth?: number | null;
}

export interface LeaveRequest {
  id: number;
  employeeId: number;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  startDate: string;
  endDate: string;
  daysRequested: number;
  status: LeaveStatus;
  reason?: string | null;
  rejectionComment?: string | null;
  createdAt: string;
}

export interface LeaveSubmission {
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  reason?: string | null;
}

export interface LeaveApproval {
  status: LeaveStatus;
  rejectionComment?: string | null;
}

export interface BulkApproval {
  requestIds: number[];
  status: LeaveStatus;
  rejectionComment?: string | null;
}

export interface LeaveBalance {
  leaveTypeId: number;
  leaveTypeName: string;
  balance: number;
  defaultDays: number;
  isAccrued: boolean;
  accruedToDate: number;
}

export interface LeaveBalanceSummary {
  employeeId: number;
  employeeName: string;
  totalBalance: number;
  balances: LeaveBalance[];
}

export interface Settlement {
  employeeId: number;
  leaveTypeId: number;
  newBalance: number;
  reason?: string | null;
}

export interface SettlementHistory {
  employeeId: number;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  previousBalance: number;
  newBalance: number;
  reason?: string | null;
  adjustedAt: string;
}

export interface LeaveFilter {
  employeeId?: number;
  status?: LeaveStatus;
  leaveTypeId?: number;
  fromDate?: string;
  toDate?: string;
}
