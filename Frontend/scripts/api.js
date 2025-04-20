

const apiClient = axios.create({
    baseURL: 'http://192.168.50.9:5228/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, 
});

export const getData = async (letter) => {
    try{
        const response = await apiClient.get('/Names/by-letter', {
            params: {
                letter
            }
        });
        return response.data;
    } catch (error) {
        console.error('Whalla din wahab! ', error);
        throw error;
    }
};