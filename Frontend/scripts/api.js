import { state, lists, Reaction } from './state.js';

const apiClient = axios.create({
    baseURL: 'http://192.168.50.9:5228/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

export const getNamesByLetter = async (letter, pageNumber = 1, minCount = 0, pageSize = 5) => {
    try {
        console.log(letter, pageNumber);
        const response = await apiClient.get('/Names/by-letter', {
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
export const loginUser = async (username, password) => {
    try {
        const response = await apiClient.post('/Account/login', {
            username: username,
            password: password,
        });
        return response; // Hämta namnplattor
    } catch (error) {
        console.error('login error ', error);
        return error.response.data;
    }
};
export const completeRegistration = async (token, email) => {
    await apiClient.post('/Account/verify-email', { email,token })
    .then(() => {
      // kontot är verifierat, redirecta till login eller nåt
      window.location.replace('http://192.168.50.9:5500/index.html');
    })
    .catch((error) => {
      // token ogiltig eller nåt kefft hände
      console.error('completeRegistration error ', error);
        window.location.replace('http://192.168.50.9:5500/index.html');
        return error.response.data;
    });
}
export const registerUser = async (username, email, password) => {
    try {
        const response = await apiClient.post('/Account/register', {
            username: username,
            email: email,
            password: password,
        });
        return response; // Hämta namnplattor
    } catch (error) {
        console.error('register error ', error);
        return error.response.data;
    }
};
export const addReactions = async (reactions) => {
    try {
        const response = await apiClient.post('/Reactions/reactions', reactions);
        return response;
    } catch (error) {
        console.error('Reaction http error ', error);
        return error.response.data;
    }
};
export const addPartnerRequest = async (partner) => {
    try {
        const response = await apiClient.post('/Account/add-partner', partner, {
            headers: {
              'Content-Type': 'application/json'
            }
          });
        return response;
    } catch (error) {
        console.error('add-partner http error ', error);
        return error.response.data;
    }
};

export const fetchReactions = async (reaction) => {
    try {
        const response = await apiClient.get('/Reactions/reactions', {
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
    } catch (error) {
        console.error('API-anropet misslyckades:', error);
    }
};

export const postReactions = async () => {
    try {
        const response = await apiClient.post('/Reactions/reactions', state.reactionsToAdd);   
        console.log("reactionsToAdd: ", response);
        lists.hasFetched.favorites = false;
        if (response.status === 200) {
            state.reactionsToAdd = [];
        } else {
            state.errorMessage = response;
        }
    } catch (error) {
        console.error('Reaction http error ', error);
        return error.response.data;
    }
};