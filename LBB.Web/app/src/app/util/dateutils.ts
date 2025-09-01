import { format } from 'date-fns';

export const toFormDate = (date: Date) => {
  return format(date, "yyyy-MM-dd'T'HH:mm:ss");
};
