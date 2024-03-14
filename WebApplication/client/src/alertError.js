async function alertError(response) {
    const text = await response.text();
    try {
        const error = JSON.parse(text);
        const message = error.message || 'Error processing your request';
        alert(message);
        return message;
    } catch (e) {
        alert(text);
        return text;
    }
}

export default alertError;