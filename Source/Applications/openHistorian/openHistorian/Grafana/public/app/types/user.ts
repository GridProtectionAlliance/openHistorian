import { TimeZone } from '@grafana/data';

export interface OrgUser {
  avatarUrl: string;
  email: string;
  lastSeenAt: string;
  lastSeenAtAge: string;
  login: string;
  name: string;
  orgId: number;
  role: string;
  userId: number;
}

export interface User {
  id: number;
  label: string;
  avatarUrl: string;
  login: string;
  email: string;
  name: string;
  orgId?: number;
}

export interface UserDTO {
  id: number;
  login: string;
  email: string;
  name: string;
  isGrafanaAdmin: boolean;
  isDisabled: boolean;
  isExternal?: boolean;
  updatedAt?: string;
  authLabels?: string[];
  theme?: string;
  avatarUrl?: string;
  orgId?: number;
}

export interface Invitee {
  code: string;
  createdOn: string;
  email: string;
  emailSent: boolean;
  emailSentOn: string;
  id: number;
  invitedByEmail: string;
  invitedByLogin: string;
  invitedByName: string;
  name: string;
  orgId: number;
  role: string;
  status: string;
  url: string;
}

export interface UsersState {
  users: OrgUser[];
  invitees: Invitee[];
  searchQuery: string;
  canInvite: boolean;
  externalUserMngLinkUrl: string;
  externalUserMngLinkName: string;
  externalUserMngInfo: string;
  hasFetched: boolean;
}

export interface UserState {
  orgId: number;
  timeZone: TimeZone;
}

export interface UserSession {
  id: number;
  createdAt: string;
  clientIp: string;
  isActive: boolean;
  seenAt: string;
  browser: string;
  browserVersion: string;
  os: string;
  osVersion: string;
  device: string;
}

export interface UserOrg {
  name: string;
  orgId: number;
  role: string;
}

export interface UserAdminState {
  user: UserDTO;
  sessions: UserSession[];
  orgs: UserOrg[];
  isLoading: boolean;
  error?: UserAdminError;
}

export interface UserAdminError {
  title: string;
  body: string;
}
