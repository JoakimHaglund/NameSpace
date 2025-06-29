export enum Reaction {
  FAVORITE = 2,
  LIKE = 1,
  DISLIKE = 0
};
const reactionTypeFromString: Record<string, Reaction> = {
    favorites: Reaction.FAVORITE,
    liked: Reaction.LIKE,
    disliked: Reaction.DISLIKE
}
const reactionTypeToString: Record<Reaction, string> = {
    [Reaction.FAVORITE]: 'favorites',
    [Reaction.LIKE]: 'liked',
    [Reaction.DISLIKE]: 'disliked' 
}
export const parseReactionType = (input: string | string[]): Reaction => {
    const reactionString = Array.isArray(input) ? input[0] : input
    const normalizedReaction = reactionString.toLowerCase();

    if(!(normalizedReaction in reactionTypeFromString)){
        throw new Error(`Invalid reaction type: '${reactionString}'`);
    }
    return reactionTypeFromString[normalizedReaction]
}
export const stringifyReactionType = (reaction: Reaction): string => {
    return reactionTypeToString[reaction]
}

export default {
    Reaction, 
    stringifyReactionType, 
    parseReactionType
}