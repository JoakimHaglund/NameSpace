

const apiClient = axios.create({
    baseURL: 'http://192.168.50.9:5228/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, 
});

export const getNamesByLetter = async (letter, pageNumber = 1, pageSize = 50) => {
    try{
        console.log(letter, pageNumber);
        const response = await apiClient.get('/Names/by-letter', {
            params: {
                letter,
                pageNumber,
                pageSize,
                minCount: 100,
            }
        });
        return response.data;
    } catch (error) {
        console.error('error getting names ', error);
        throw error;
    }
};
export const loginUser = async (username, password) => {
    try{
        const response = await apiClient.post('/Account/login', {
          username: username,
          password: password,
        });
        return response; // Hämta namnplattor
    }catch (error) {
        console.error('login error ', error);
        return error.response.data;
    }
};
export const addReactions = async (reactions) => {
    try{
        const response = await apiClient.post('/Reactions/reactions', reactions);
        return response;
    }catch (error) {
        console.error('Reaction http error ', error);
        return error.response.data;
    }
};
export const getReactions = async (reaction) => {
    try{
        const response = await apiClient.get('/Reactions/reactions', {
            params: {
                reaction: reaction,
            }
        });
        return response.data;
    } catch (error) {
        console.error('error getting names ', error);
        throw error;
    }
};