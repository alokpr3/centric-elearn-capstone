import type { ReactNode } from 'react';

interface Props {
  loading: boolean;
  error: string | null;
  empty?: boolean;
  emptyText?: string;
  children: ReactNode;
}

export default function PageState({ loading, error, empty, emptyText = 'Nothing to show.', children }: Props) {
  if (loading) return <p className="state">Loading...</p>;
  if (error) return <p className="state state-error">{error}</p>;
  if (empty) return <p className="state">{emptyText}</p>;
  return <>{children}</>;
}
