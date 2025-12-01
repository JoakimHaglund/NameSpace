import { state, lists, type NameInfo } from './state.js';
import { Reaction } from './reactionType.js';
import axios from 'axios'

const apiClient = axios.create({
    baseURL: 'http://192.168.50.9:5228/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});
export const checkLoginStatus = async () => {
    try {
        await apiClient.get('http://192.168.50.9:5228/api/account/check-login');
        state.isLoggedIn = true;
        //fetchData(nameQuery.letter, nameQuery.pagenum);
    } catch (error) {
        state.isLoggedIn = false;
    } finally {
        state.isCheckingLogin = false;
    }
};
export const getNamesByLetter = async (letter: string, pageNumber = 1, minCount = 0, pageSize = 5) => {
    try {
        console.log(letter, pageNumber);
        const response = await apiClient.get('/names/by-letter', {
            params: {
                letter,
                pageNumber,
                pageSize,
                minCount: minCount,
            }
        });
        return response.data;
    } catch (error) {
        console.error('error getting names ', error);
        throw error;
    }
};
export const logout = () => {
    apiClient.post('account/logout')
        .then(() => {
            state.isLoggedIn = false;
            // window.location.href = '/login';  // eller använd vue-router för redirect
        }).catch((error) => {
            console.error("Logout failed:", error);
        });
};
export const loginUser = async (username: string, password: string) => {
    try {
        const response = await apiClient.post('/account/login', {
            username: username,
            password: password,
        });
        return response; // Hämta namnplattor
    } catch (error: unknown) {
        console.error('login error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const completeRegistration = async (token: string, email: string) => {
    try {
        const response = await apiClient.post('/Account/verify-email', { email, token })

        return response.data
    }
    catch(error: any) {
        console.error('completeRegistration error ', error);
        return error.response.data;
    };
}
export const registerUser = async (username: string, email: string, password: string) => {
    try {
        const response = await apiClient.post('/account/register', {
            username: username,
            email: email,
            password: password,
        });
        return response; // Hämta namnplattor
    } catch (error) {
        console.error('register error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const addReactions = async (reactions: string) => {
    try {
        const response = await apiClient.post('/Reactions/reactions', reactions);
        return response;
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const addPartnerRequest = async (partner: string) => {
    try {
        const response = await apiClient.post('/account/add-partner', partner, {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response;
    } catch (error) {
        console.error('add-partner http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const fetchReactions = async (reaction: number) => {
    try {
        const response = await apiClient.get('/reactions/reactions', {
            params: {
                reaction: reaction,
            }
        });

        if (reaction === Reaction.FAVORITE) {
            lists.favorites = [...response.data];
            lists.hasFetched.favorites = true;
        }
        else if (reaction === Reaction.LIKE) {
            lists.liked = [...response.data];
            lists.hasFetched.liked = true;
        }
        else if (reaction === Reaction.DISLIKE) {
            lists.disliked = [...response.data];
            lists.hasFetched.disliked = true;
        }
        console.log(lists)
        return response.data;
    } catch (error) {
        console.error('API-anropet misslyckades:', error);
    }
};
export const postReactions = async () => {
    try {
        const response = await apiClient.post('/reactions/reactions', state.reactionsToAdd);
        console.log("reactionsToAdd: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {
            state.reactionsToAdd = [];
        } else {
            state.errorMessage = response.data.message;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const deleteReactions = async (reactionsToDelete: NameInfo[]) => {
    try {
        const response = await apiClient.delete('/reactions', { data: reactionsToDelete});
        console.log("deleteReactions: ", response);
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const patchReactions = async (reactionsToUpdate: NameInfo[]) => {
    try {
        const response = await apiClient.patch('/reactions', reactionsToUpdate);
        console.log("patchReactions: ", response);
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const postPartnerRequest = async (partner: string) => {
    try {
        const response = await apiClient.post('/account/request-partner', {
            params: {
                partner: partner,
            }
        });
        console.log("request-partner: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {

        } else {
            state.errorMessage = response.data.message;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const postAddPartner = async (partnerRequestId: string) => {
    try {
        const response = await apiClient.post('/account/add-partner', partnerRequestId);
        console.log("add-partner: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {

        } else {
            state.errorMessage = response.data.message;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const fetchPartnerRequests = async () => {
    try {
        const response = await apiClient.get('/account/partner-requests');
        console.log("partner-requests: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {
            return response.data;
        } else {
            state.errorMessage = response.data.message;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};
export const deletePartner = async (partnerRequestId: number) => {
    try {
        const response = await apiClient.delete('/account/partner-request', {
            params: { partnerRequestId }
        });
        console.log("reactionsToAdd: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {
            state.reactionsToAdd = [];
        } else {
            state.errorMessage = response.data.message;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        if (axios.isAxiosError(error)) {
            return error.response?.data;
        }
    }
};