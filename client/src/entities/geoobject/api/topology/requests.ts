import { toast } from 'react-toastify';

import { post } from '../../../../shared/lib/fetch';
import type { ParentChildObjectLink } from '../../model/types';

export const addParentChildLinkRequest = async (params: {
    parentGeographicalObjectId: string;
    childGeographicalObjectId: string;
}) => {
    try {
        const data = await post<ParentChildObjectLink>('/api/ParentChild/Link', { body: params });
        return data;
    } catch (error) {
        toast(`Не получилось добавить связь, проверьте введенные id`, { type: 'error' });
        console.error(error);
    }
};

export const addTopologyRequest = async (params: {
    geographicalObjectInId: string;
    geographicalObjectOutId: string;
}) => {
    try {
        const data = await post<ParentChildObjectLink[]>('/api/Topology', { body: params });

        return data;
    } catch (error) {
        toast(`Не получилось добавить связь, проверьте введенные id`, { type: 'error' });
        console.error(error);
    }
};
